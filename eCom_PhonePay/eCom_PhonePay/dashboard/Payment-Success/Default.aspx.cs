
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace eCom_IMEPay.dashboard.Payment_Success
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string PRN = Request.QueryString.Get("PRN");
            string BC = Request.QueryString.Get("BC");

            if (HttpUtility.UrlDecode(BC) == "N/A")
            {
                Response.Redirect(Page.Request.Url.Scheme + "://" + Request.Url.Authority);
            }
            else
            {
                if (PRN.Length > 0)
                    Compute(PRN);
            }
        }

        private void Compute(string txnID)
        {
            string merchantCode = "";
            string key = "";

            string PID, PRN, BID, UID;
            PRN = Request.QueryString.Get("PRN");
            UID = Request.QueryString.Get("UID");
            BID = Request.QueryString.Get("BID");
            PID = merchantCode;
            double amount = 1;

            string message = string.Format("{0},{1},{2},{3},{4}", PID, amount, PRN, BID, UID);

            string hashHMACHex = GenerateHMACHash(key, message);

            string verificationUrl = "https://clientapi.fonepay.com/api/merchantRequest/verificationMerchant";

            string query = string.Format("?PRN={0}&PID={1}&BID={2}&AMT={3}&UID={4}&DV={5}", PRN, PID, BID, amount, UID, hashHMACHex);

            verificationUrl = verificationUrl + query;
            var req = (HttpWebRequest)WebRequest.Create(verificationUrl);

            req.Method = "GET";

            var stIn = new StreamReader(req.GetResponse().GetResponseStream());
            string _strResponse = stIn.ReadToEnd();
            stIn.Close();

            var fonePayObj = XmlConvert.DeserializeObject<fonePayResult>(_strResponse);


            

            lbl.Text = fonePayObj.success;

        }

        private static string GenerateHMACHash(string keyHex, string message)
        {
            byte[] hash = HashHMAC(StringEncode(keyHex), StringEncode(message));
            return HashEncode(hash);
        }

        private static byte[] StringEncode(string text)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(text);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA512(key);
            return hash.ComputeHash(message);
        }


    }

    public static class XmlConvert
    {
        public static string SerializeObject<T>(T dataObject)
        {
            if (dataObject == null)
            {
                return string.Empty;
            }
            try
            {
                using (StringWriter stringWriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, dataObject);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static T DeserializeObject<T>(string xml)
             where T : new()
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new T();
            }
            try
            {
                using (var stringReader = new StringReader(xml))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception ex)
            {
                return new T();
            }
        }
    }



    [XmlRoot("response")]
    public class fonePayResult
    {
        [XmlElement("amount")]
        public string amount { get; set; }
        [XmlElement("success")]
        public string success { get; set; }
        [XmlElement("txnAmount")]
        public string txnAmount { get; set; }
    }


}