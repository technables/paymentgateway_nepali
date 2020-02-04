using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace ecom_prabhupay
{
    public partial class prabhupay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string txnID = Request.QueryString.Get("TXNID");

            ProcessCheckout(txnID);
        }

        void ProcessCheckout(string txnID)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
            CallWebService(txnID);
        }

        public string CallWebService(string txnID)
        {
            var _url = "https://payment.prabhupay.com/Api/PGateway.svc";
            //var _action = "http://tempuri.org/IPGateway/GetGatewayList";
            var _action = "http://tempuri.org/IPGateway/InitializeTransaction";

            var soapEnvelopeXml = CreateInitialTransactionEnvelope(txnID);
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

                var res = Deserialize<TransactionEnvelope>(soapResult);

                if (res.Body.Response.Result.Code == "000")
                {
                    Response.Redirect(res.Body.Response.Result.ProcessURL);
                }
                else
                {
                    lbl.Text = res.Body.Response.Result.Message;
                }

                return soapResult;
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

        private static XmlDocument CreateGatewayEnvelope()
        {
            string username = "";
            string password = "";
            string secret = "";

            string soapString = @"<?xml version=""1.0"" encoding=""utf-8""?>
          <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:pgat=""http://schemas.datacontract.org/2004/07/PGateway.API"">
              <soapenv:Body>
                   <tem:GetGatewayList>
                     <tem:GetGatewayListRequest>
                        <pgat:UserName>#username#</pgat:UserName>
                        <pgat:Password>#password#</pgat:Password>
                        <pgat:ApiSecret>#secret#</pgat:ApiSecret>
                     </tem:GetGatewayListRequest>
                  </tem:GetGatewayList>
              </soapenv:Body>
          </soapenv:Envelope>";

            soapString = soapString.Replace("#username#", username).Replace("#password#", password).Replace("#secret#", secret);

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(soapString);
            return soapEnvelopeDocument;
        }

        private XmlDocument CreateInitialTransactionEnvelope(string txnID)
        {
            string username = "";
            string password = "";
            string secret = "";

            string soapString = @"<?xml version=""1.0"" encoding=""utf-8""?>
          <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"" xmlns:pgat=""http://schemas.datacontract.org/2004/07/PGateway.API"">
              <soapenv:Body>
                   <tem:InitializeTransaction>
                     <tem:InitializeTransactionRequest>
                        <pgat:UserName>#username#</pgat:UserName>
                        <pgat:Password>#password#</pgat:Password>
                        <pgat:ApiSecret>#secret#</pgat:ApiSecret>
                        <pgat:GatewayIds>#gateways#</pgat:GatewayIds>
                        <pgat:ReturnURL>#returnurl#</pgat:ReturnURL>
                        <pgat:Amount>#amount#</pgat:Amount>
                        <pgat:Remarks>#remark#</pgat:Remarks>
                        <pgat:CustName>#custname#</pgat:CustName>
                        <pgat:CustEmail>#email#</pgat:CustEmail>
                        <pgat:CustMobile>#custmobile#</pgat:CustMobile>
                        <pgat:CustIPAddress>#ipaddress#</pgat:CustIPAddress>
                        <pgat:PartnerTxnId>#txnID#</pgat:PartnerTxnId>
                     </tem:InitializeTransactionRequest>
                  </tem:InitializeTransaction>
              </soapenv:Body>
          </soapenv:Envelope>";

            string hostUrl = Page.Request.Url.Scheme + "://" + Request.Url.Authority + (Request.ApplicationPath == "/" ? "" : Request.ApplicationPath);

            string returnUrl = hostUrl + "/dashboard/Payment-Success/Default.aspx";

            soapString = soapString.Replace("#username#", username).Replace("#password#", password).Replace("#secret#", secret);
            soapString = soapString.Replace("#gateways#", "");
            soapString = soapString.Replace("#returnurl#", returnUrl);
            soapString = soapString.Replace("#amount#", "100");
            soapString = soapString.Replace("#remark#", "remarks");
            soapString = soapString.Replace("#custname#", "chandra dev singh");
            soapString = soapString.Replace("#email#", "amarchandr@gmial.com");
            soapString = soapString.Replace("#custmobile#", "9851160059");
            soapString = soapString.Replace("#ipaddress#", "110.44.123.19");
            soapString = soapString.Replace("#txnID#", txnID);

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(soapString);
            return soapEnvelopeDocument;
        }

        private XmlDocument CreateTransactionStatusEnvelope()
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

            string hostUrl = Page.Request.Url.Scheme + "://" + Request.Url.Authority + (Request.ApplicationPath == "/" ? "" : Request.ApplicationPath);


            soapString = soapString.Replace("#username#", username).Replace("#password#", password).Replace("#secret#", secret);

            soapString = soapString.Replace("#txnID#", "3262104");

            XmlDocument soapEnvelopeDocument = new XmlDocument();
            soapEnvelopeDocument.LoadXml(soapString);
            return soapEnvelopeDocument;
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

    }



}