
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ecom_prabhupay.dashboard.Payment_Success
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string Code = Request.Form["Code"];
            string message = Request.Form["Message"];
            string txnID = Request.Form["PartnerTxnId"];

            if (Code == "000" || Code == "777")
            {

                Compute();
            }
            else
            {
                lbl.Text = message;

            }


        }

        private void Compute()
        {

            string Code = Request.Form["Code"];
            string message = Request.Form["Message"];
            string txnID = Request.Form["PartnerTxnId"];

            GetTransactionStatusResult response = CallWebService(txnID);

            if (response.Code == "000" || response.Code == "777")
            {
                lbl.Text = ("Congratulations!! your payment is successful.!!");
            }
            else
            {
                lbl.Text = string.Format(response.Message);
            }


        }

        public GetTransactionStatusResult CallWebService(string txnID)
        {
            var _url = "https://testpayment.prabhupay.com/Api/PGateway.svc";
            var _action = "http://tempuri.org/IPGateway/GetTransactionStatus";

            var soapEnvelopeXml = CreateTransactionStatusEnvelope(txnID);
            var soapRequest = CreateSoapRequest(_url, _action);
            InsertSoapEnvelopeIntoSoapRequest(soapEnvelopeXml, soapRequest);

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    soapEnvelopeXml.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                }
            }

            // begin async call to web request.
            var asyncResult = soapRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete. You might want to
            // do something usefull here like update your UI.
            var success = asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

            if (!success) return null;

            // get the response from the completed web request.
            using (var webResponse = soapRequest.EndGetResponse(asyncResult))
            {
                string soapResult;
                var responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                {
                    return null;
                }
                using (var reader = new StreamReader(responseStream))
                {
                    soapResult = reader.ReadToEnd();
                }

                var res = Deserialize<TransactionStatusEnvelope>(soapResult);



                return res.Body.Response.Result;
            }
        }

        private static HttpWebRequest CreateSoapRequest(string url, string action)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static void InsertSoapEnvelopeIntoSoapRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        public static T Deserialize<T>(string input) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        private XmlDocument CreateTransactionStatusEnvelope(string txnID)
        {
             string username = "";
            string password = "";
            string secret = "";

            string soapString = @"<?xml version=""1.0"" encoding=""utf-8""?>
          <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:pgat=""http://schemas.datacontract.org/2004/07/PGateway.API"">
              <soapenv:Body>
                   <tem:GetTransactionStatus>
                     <tem:GetTransactionStatusRequest>
                        <pgat:UserName>#username#</pgat:UserName>
                        <pgat:Password>#password#</pgat:Password>
                        <pgat:ApiSecret>#secret#</pgat:ApiSecret>                        
                        <pgat:PartnerTxnId>#txnID#</pgat:PartnerTxnId>
                     </tem:GetTransactionStatusRequest>
                  </tem:GetTransactionStatus>
              </soapenv:Body>
          </soapenv:Envelope>";


            soapString = soapString.Replace("#username#", username).Replace("#password#", password).Replace("#secret#", secret);

            soapString = soapString.Replace("#txnID#", txnID);

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(soapString);
            return soapEnvelopeDocument;
        }

    }


}