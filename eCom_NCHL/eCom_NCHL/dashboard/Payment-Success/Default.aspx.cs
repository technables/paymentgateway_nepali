
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace eCom_NCHL.dashboard.Payment_Success
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Response.Write("hello");
            // lbl.Text = "hello";
            string txnID = "75";//Request.QueryString.Get("TXNID");

            Compute();
        }

        private void Compute()
        {
            string txnID = Request.QueryString.Get("TXNID");

            string url = "https://uat.connectips.com:7443/connectipswebws/api/creditor/validatetxn";
            string merchantID = "";
            string appID = "";
            string password = "";


            double orderAmount = 15000;
            string referenceID = txnID;


            string message = string.Format("MERCHANTID={0},APPID={1},REFERENCEID={2},TXNAMT={3}", merchantID, appID, referenceID, orderAmount);
            
            string certPath = Server.MapPath("/ONCECINEMA.pfx");
            string certPass = "123";
            string token = Sign(message, certPath, certPass);

            var data = new
            {
                merchantId = merchantID,
                appId = appID,
                referenceId = referenceID,
                txnAmt = orderAmount.ToString(),
                token = token
            };

            var json = JsonConvert.SerializeObject(data);

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(appID + ":" + password));
            string ContentType = "application/json";
            string result = string.Empty;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    httpClient.BaseAddress = new Uri(url);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", svcCredentials);

                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    HttpContent content = new StringContent(json, Encoding.UTF8, ContentType);
                    HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                    result = response.Content.ReadAsStringAsync().Result;

                }
                catch (Exception ex)
                {

                }
            }

            if (result.Length > 0)
            {
                ConnectIPSValidationInfo res = JsonConvert.DeserializeObject<ConnectIPSValidationInfo>(result);
                if (res != null)
                {
                    if (res.status == "SUCCESS")
                    {
                        lbl.Text = ("Congratulations!! your payment is successful.!!");
                    }
                    else if (res.status.ToLower() == "error")
                    {
                        lbl.Text = string.Format("sorry, your payment could not be validated. \n {0}", json);
                    }
                }
            }


        }

        public static HttpClient GetClient(string username, string password)
        {
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));

            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue }
                //Set some other client defaults like timeout / BaseAddress
            };
            return client;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }

        /// <summary>
        /// generate hash using certificate
        /// </summary>
        /// <param name="text">text to hash</param>
        /// <param name="certSubject">name of certificate</param>
        /// <returns>hashed string</returns>
        string Sign(string stringToHash, string pfxCertificate, string pfxPassword)
        {

            var crypt = new SHA256Managed();
            string digest = String.Empty;
            byte[] data = Encoding.UTF8.GetBytes(stringToHash);
            byte[] crypto = crypt.ComputeHash(data);

            foreach (byte theByte in crypto)
            {
                digest += theByte.ToString("x2");
            }

            X509Certificate2 cert = new X509Certificate2(pfxCertificate, pfxPassword, X509KeyStorageFlags.Exportable);
            RSACryptoServiceProvider csp = null;
            if (cert != null)
            {
                csp = cert.PrivateKey as RSACryptoServiceProvider;
            }

            if (csp == null)
            {
                throw new Exception("No valid cert was found");
            }

            csp.ImportParameters(csp.ExportParameters(true));
            byte[] signatureByte = csp.SignData(data, "SHA256");
            string signature = Convert.ToBase64String(signatureByte);

            return signature;

            //OVZwF7CJK0dv3RUs5QEFnuj/8CDk88jkskVFHI/vG3T5ihIv9IUnqxactRElvUyUQ0zrejFrMlvKkQY0olC1x74Oep5P6TmFfVIYwDE8KGjoxnlxcIrtl3P6DYX9xH1InyCKSsKcj8tZthFarQEeWAWe8bkHxb0H9MzZoEGjxbkdbj4o4dzaIF3X5V+q7jHNwKKlRkNOprgjRU3V86sWpTqqNqKxVWXR6wstyIfqaA6BduWv/JnqfhJDC/scyN7HMiEppMt9ttFWfW5WbLNN0f1RG7GbWzH6mZjwOOTf38IbaIsAUY1/9A1ta8LtTkfflYo3V+S4LAUD6gnYQol5tQ==
            //


        }
    }

    public class ConnectIPSValidationInfo
    {
        public string merchantId { get; set; }
        public string appId { get; set; }
        public string referenceId { get; set; }
        public string txnAmt { get; set; }
        public string token { get; set; }
        public string status { get; set; }
        public string statusDesc { get; set; }
    }
}