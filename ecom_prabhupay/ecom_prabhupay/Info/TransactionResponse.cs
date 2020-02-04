using System.Xml.Serialization;

namespace ecom_prabhupay
{
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class TransactionEnvelope
    {
        [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public TransactionBody Body { get; set; }
    }

    public class TransactionBody
    {
        [XmlElement("InitializeTransactionResponse", Namespace = "http://tempuri.org/")]
        public InitializeTransactionResponse Response;
    }


    public class InitializeTransactionResponse
    {
        [XmlElement("InitializeTransactionResult")]
        public InitializeTransactionResult Result;
    }

    public class InitializeTransactionResult
    {
        [XmlElement("Code", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Code { get; set; }
        [XmlElement("Message", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Message { get; set; }
        [XmlElement("TransactionId", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string TransactionId { get; set; }
        [XmlElement("ProcessURL", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string ProcessURL { get; set; }
    }    
}