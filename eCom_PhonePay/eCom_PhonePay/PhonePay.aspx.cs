using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;
using System.Web;

namespace eCom_IMEPay
{
    public partial class PhonePay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string txnID = Request.QueryString.Get("TXNID");
            ProcessPhonePayCheckout(txnID);
        }

        private void ProcessPhonePayCheckout(string txtID)
        {

            string merchantCode = "";
            string key = "";
            string mode = "P";
            string CRN = "NPR";

            string prn = txtID; //bookingno
            string amount = "1";//booking amount;
            string date = DateTime.Now.ToString("MM/dd/yyyy");
            string R1 = "12"; //bookingID
            string R2 = "test";
            string RU = Page.Request.Url.Scheme + "://" + Request.Url.Authority + "/dashboard/Payment-Success";

            string message = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", merchantCode, mode, prn, amount, CRN, date, R1, R2, RU);
            string hashHMACHex = GenerateHMACHash(key, message);

            string requestURL = "https://clientapi.fonepay.com/api/merchantRequest";

            string query = string.Format("?PID={0}&MD={1}&AMT={2}&CRN={3}&DT={4}&R1={5}&R2={6}&DV={7}&RU={8}&PRN={9}", merchantCode, mode, amount, CRN, URLEncode(date), URLEncode(R1), URLEncode(R2), hashHMACHex, RU, prn);

            requestURL = requestURL + query;
            Response.Redirect(requestURL, false);
        }

        public string URLEncode(string data)
        {
            return HttpUtility.UrlEncode(data);
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


}