using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eCom_IMEPay
{
    public partial class IMEPay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string txnID = Request.QueryString.Get("TXNID");
            ProcessIMEPayCheckout("1");
        }

        void ProcessIMEPayCheckout(string txnID)
        {
            string tokenUrl = "http://202.166.194.123:7979/api/Web/GetToken";
            string MerchantCode = "";
            string ApiUser = "";
            string Password = "";
            string Module = "";


            long orderID = long.Parse("10");
            double orderAmount = 11.69;

            string authHeader = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ApiUser + ":" + Password));
            string module = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Module));

            var data = new
            {
                MerchantCode = MerchantCode,
                RefId = "Ref-9901",
                Amount = orderAmount
            };

            var json = JsonConvert.SerializeObject(data);

            string ContentType = "application/json";
            string result = string.Empty;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri(tokenUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                    httpClient.DefaultRequestHeaders.Add("Module", module);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    HttpContent content = new StringContent(json, Encoding.UTF8, ContentType);
                    HttpResponseMessage response = httpClient.PostAsync(tokenUrl, content).Result;
                    result = response.Content.ReadAsStringAsync().Result;

                }
                catch (Exception ex)
                {

                }
            }

            if (result.Length > 0)
            {
                IMEPayTokenInfo res = JsonConvert.DeserializeObject<IMEPayTokenInfo>(result);
                if (res != null)
                {
                    if (res.ResponseCode == 0)
                    {
                        //update tokenid in transactionlog
                        string loginUrl = "http://202.166.194.123:7979/WebCheckout/Checkout";
                        Response.Clear();
                        var sb = new System.Text.StringBuilder();
                        sb.Append("<html>");
                        sb.AppendFormat("<body onload='document.forms[0].submit()'>");
                        sb.AppendFormat("<form action='{0}' method='post'>", loginUrl);
                        sb.AppendFormat("<input type='hidden' type='text' name='TokenId' id='TokenId' value='{0}' />", res.TokenId);
                        sb.AppendFormat("<input type='hidden' type='text' name='MerchantCode' id='MerchantCode' value='{0}' />", MerchantCode);
                        sb.AppendFormat("<input type='hidden' type='text' name='RefId' id='RefId' value='{0}' />", orderID);
                        sb.AppendFormat("<input type='hidden' type='text' name='TranAmount' id='TranAmount' value='{0}' />", orderAmount);
                        sb.AppendFormat("<input type='hidden' type='text' name='Source' id='Source' value='{0}' />", "W");

                        sb.Append("</form>");
                        sb.Append("</body>");
                        sb.Append("</html>");
                        Response.Write(sb.ToString());
                        Response.End();


                    }
                    else
                    {
                        //lbl.Text = string.Format("sorry, your payment could not be validated. \n {0}", json);
                    }
                }
            }

        }
    }

    public class IMEPayTokenInfo
    {
        public int ResponseCode { get; set; }
        public string TokenId { get; set; }
        public string Amount { get; set; }
        public string RefId { get; set; }
    }
}