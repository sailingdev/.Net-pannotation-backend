using System.Xml.Serialization;

namespace Pannotation.Models.RequestModels.Payment
{
    [XmlRoot("AuthorizationResponseData")]
    public class ReturnedRecurringPayment
    {
        [XmlElement("ResponseCode")]
        public string ResponseCode { get; set; }

        [XmlElement("ReferenceNo")]
        public string ReferenceNo { get; set; }

        [XmlElement("AuthId")]
        public string AuthId { get; set; }

        [XmlElement("RecurringAdviceIndicator")]
        public string RecurringAdviceIndicator { get; set; }

        [XmlElement("RecurringPeriod")]
        public string RecurringPeriod { get; set; }

        [XmlElement("CardExpiring")]
        public string CardExpiring { get; set; }

        [XmlElement("OriginalResponseCode")]
        public string OriginalResponseCode { get; set; }

        [XmlElement("OrderId")]
        public string OrderId { get; set; }

        [XmlElement("MerchantId")]
        public string MerchantId { get; set; }

        [XmlElement("AttemptNumber")]
        public string AttemptNumber { get; set; }

        [XmlElement("ReasonCode")]
        public string ReasonCode { get; set; }

        [XmlElement("ReasonCodeDesc")]
        public string ReasonCodeDesc { get; set; }
    }
}
