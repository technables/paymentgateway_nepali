using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ecom_prabhupay
{
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class TransactionStatusEnvelope
    {
        [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public TransactionStatusBody Body { get; set; }
    }

    public class TransactionStatusBody
    {
        [XmlElement("GetTransactionStatusResponse", Namespace = "http://tempuri.org/")]
        public GetTransactionStatusResponse Response;
    }


    public class GetTransactionStatusResponse
    {
        [XmlElement("GetTransactionStatusResult")]
        public GetTransactionStatusResult Result;
    }

    public class GetTransactionStatusResult
    {
        [XmlElement("Code", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Code { get; set; }
        [XmlElement("Message", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Message { get; set; }
        [XmlElement("TransactionId", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string TransactionId { get; set; }
        [XmlElement("GatewayId", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string GatewayId { get; set; }
        [XmlElement("GatewayName", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string GatewayName { get; set; }
        [XmlElement("PartnerTxnId", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string PartnerTxnId { get; set; }
        [XmlElement("Amount", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Amount { get; set; }
        [XmlElement("Scharge", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Scharge { get; set; }
        [XmlElement("NetAmount", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string NetAmount { get; set; }

    }
}