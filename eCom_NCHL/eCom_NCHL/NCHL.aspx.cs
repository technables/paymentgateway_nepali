using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace eCom_NCHL
{
    public partial class NCHL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string txnID = Request.QueryString.Get("TXNID");
           
            ProcessNCHLCheckout(txnID);
            //Compute();
        }
        private void ProcessNCHLCheckout(string txnID)
        {
            string merchantID = "229";
            string appID = "MER-229-APP-1";
            string appName = "One cinemas";

            long orderID = long.Parse(txnID);
            string orderDate = "11-26-2019";
            string currency = "NPR";
            double orderAmount = 15000;
            string referenceID = "ref-" + orderID;
            string remarks = "RMKS-" + orderID;
            string particulars = "test";

            string message = "MERCHANTID={0},APPID={1},APPNAME={2},TXNID={3},TXNDATE={4},TXNCRNCY={5},TXNAMT={6},REFERENCEID={7},REMARKS={8},PARTICULARS={9},TOKEN=TOKEN";
            string stringToHash = string.Format(message, merchantID, appID, appName, orderID, orderDate, currency, orderAmount, referenceID, remarks, particulars);

            //stringToHash = "MERCHANTID=229,APPID=MER-229-APP-1,REFERENCEID=78,TXNAMT=15000.0";

            string certPath = Server.MapPath("/ONCECINEMA.pfx");
            string certPass = "123";


            //stringToHash = "MERCHANTID=1,APPID=MER-1-APP-1,APPNAME=Inland Revenue Department,TXNID=8024,TXNDATE=08-10-2017,TXNCRNCY=NPR,TXNAMT=1000,REFERENCEID=1.2.4,REMARKS=123455,PARTICULARS=12345,TOKEN=TOKEN";


            string token = Sign(stringToHash, certPath, certPass);


            //form post to navigate to connectips login page
            string url = "https://uat.connectips.com:7443/connectipswebgw/loginpage";
            Response.Clear();
            var sb = new System.Text.StringBuilder();
            sb.Append("<html>");
            sb.AppendFormat("<body onload='document.forms[0].submit()'>");
            sb.AppendFormat("<form action='{0}' method='post'>", url);
            sb.AppendFormat("<input type='hidden' type='text' name='MERCHANTID' id='MERCHANTID' value='{0}' />", merchantID);
            sb.AppendFormat("<input type='hidden' type='text' name='APPID' id='APPID' value='{0}' />", appID);
            sb.AppendFormat("<input type='hidden' type='text' name='APPNAME' id='APPNAME' value='{0}' />", appName);
            sb.AppendFormat("<input type='hidden' type='text' name='TXNID' id='TXNID' value='{0}' />", orderID);
            sb.AppendFormat("<input type='hidden' type='text' name='TXNDATE' id='TXNDATE' value='{0}' />", orderDate);
            sb.AppendFormat("<input type='hidden' type='text' name='TXNCRNCY' id='TXNCRNCY' value='{0}' />", currency);
            sb.AppendFormat("<input type='hidden' type='text' name='TXNAMT' id='TXNAMT' value='{0}' />", orderAmount);
            sb.AppendFormat("<input type='hidden' type='text' name='REFERENCEID' id='REFERENCEID' value='{0}' />", referenceID);
            sb.AppendFormat("<input type='hidden' type='text' name='REMARKS' id='REMARKS' value='{0}' />", remarks);
            sb.AppendFormat("<input type='hidden' type='text' name='PARTICULARS' id='PARTICULARS' value='{0}' />", particulars);
            sb.AppendFormat("<input type='hidden' type='text' name='TOKEN' id='TOKEN' value='{0}' />", token);

            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");
            Response.Write(sb.ToString());
            Response.End();

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
}