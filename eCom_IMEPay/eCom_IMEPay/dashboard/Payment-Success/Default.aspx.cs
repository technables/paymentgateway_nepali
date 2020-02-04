
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace eCom_IMEPay.dashboard.Payment_Success
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string responseCode = Request.Form["ResponseCode"];
            string RefId = Request.Form["RefId"];

            if (responseCode == "0")
            {
                Compute(RefId);
            }
            else if (responseCode == "1")
            {
                lbl.Text = "your payment request failed";

            }
            else if (responseCode == "2")
            {
                lbl.Text = "error in payment";
            }
            else if (responseCode == "3")
            {
                Response.Redirect("/");
            }
            else
            {
                RetryPayment(RefId);
            }

        }

        private void Compute(string txnID)
        {

            string responseCode = Request.Form["ResponseCode"];
            string RefId = Request.Form["RefId"];
            string TranAmount = Request.Form["TranAmount"];
            string Msisdn = Request.Form["Msisdn"];
            string TransactionId = Request.Form["TransactionId"];


            string verificationUrl = "http://202.166.194.123:7979/api/Web/Confirm";
            string MerchantCode = "";
            string ApiUser = "";
            string Password = "";
            string Module = "";

            string authHeader = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ApiUser + ":" + Password));
            string module = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Module));

            var data = new
            {
                TransactionId = TransactionId,
                MerchantCode = MerchantCode,
                Msisdn = Msisdn,
                RefId = RefId
            };

            var json = JsonConvert.SerializeObject(data);

            string ContentType = "application/json";
            string result = string.Empty;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri(verificationUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                    httpClient.DefaultRequestHeaders.Add("Module", module);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    HttpContent content = new StringContent(json, Encoding.UTF8, ContentType);
                    HttpResponseMessage response = httpClient.PostAsync(verificationUrl, content).Result;
                    result = response.Content.ReadAsStringAsync().Result;

                }
                catch (Exception ex)
                {

                }
            }

            if (result.Length > 0)
            {
                IMEPayVerificationInfo res = JsonConvert.DeserializeObject<IMEPayVerificationInfo>(result);
                if (res != null)
                {
                    if (res.ResponseCode == 0)
                    {
                        lbl.Text = ("Congratulations!! your payment is successful.!!");
                    }
                    else if (res.ResponseCode == 1)
                    {
                        lbl.Text = string.Format("sorry, your payment has failed. \n {0}", json);
                    }
                    else
                    {
                        lbl.Text = string.Format("sorry, your payment could not be validated. \n {0}", json);
                    }
                }
            }

        }

        private void RetryPayment(string RefId)
        {

            string MerchantCode = "";
            string ApiUser = "";
            string Password = "";
            string Module = "";

            string TokenID = "TokenId";

            string authHeader = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ApiUser + ":" + Password));
            string module = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Module));

            var data = new
            {
                MerchantCode = MerchantCode,
                RefId = RefId,
                TokenId = TokenID
            };

            var json = JsonConvert.SerializeObject(data);

            string ContentType = "application/json";
            string result = string.Empty;

            string verificationUrl = "http://202.166.194.123:7979/api/Web/Recheck";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri(verificationUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                    httpClient.DefaultRequestHeaders.Add("Module", module);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    HttpContent content = new StringContent(json, Encoding.UTF8, ContentType);
                    HttpResponseMessage response = httpClient.PostAsync(verificationUrl, content).Result;
                    result = response.Content.ReadAsStringAsync().Result;

                }
                catch (Exception ex)
                {

                }
            }

            if (result.Length > 0)
            {
                IMEPayVerificationInfo res = JsonConvert.DeserializeObject<IMEPayVerificationInfo>(result);
                if (res != null)
                {
                    if (res.ResponseCode == 0)
                    {
                        lbl.Text = ("Congratulations!! your payment is successful.!!");
                    }
                    else
                    {
                        lbl.Text = string.Format("sorry, your payment could not be validated. \n {0}", json);
                    }
                }
            }


        }
    }

    public class IMEPayVerificationInfo
    {
        public int ResponseCode { get; set; }
        public string Msisdn { get; set; }
        public string TransactionId { get; set; }
        public string ResponseDescription { get; set; }
    }

}