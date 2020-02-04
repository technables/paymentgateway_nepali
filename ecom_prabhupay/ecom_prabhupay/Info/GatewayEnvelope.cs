using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ecom_prabhupay
{
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class GatewayEnvelope
    {
        [XmlElement("Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body Body { get; set; }
    }

    public class Body
    {
        [XmlElement("GetGatewayListResponse", Namespace = "http://tempuri.org/")]
        public GetGatewayListResponse Response;
    }


    public class GetGatewayListResponse
    {
        [XmlElement("GetGatewayListResult")]
        public GetGatewayListResult Result;
    }

    public class GetGatewayListResult
    {
        [XmlElement("Code", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Code { get; set; }
        [XmlElement("Message", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Message { get; set; }
        [XmlElement("Gateways", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public Gateways Gateways { get; set; }
    }

    public class Gateways
    {
        [XmlElement("Gateway", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public Gateway[] GetwayList;
    }

    public class Gateway
    {
        [XmlElement("Id", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public int Id { get; set; }
        [XmlElement("Name", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Name { get; set; }
        [XmlElement("Type", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string Type { get; set; }
        [XmlElement("LogoURL", Namespace = "http://schemas.datacontract.org/2004/07/PGateway.API")]
        public string LogoURL { get; set; }
    }
}