using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pannotation.Models.Enums;
using System;

namespace Pannotation.Models.ResponseModels.Payment
{
    public class TransactionInfoResponseModel
    {
        [JsonProperty("cardMask")]
        public string CardMask { get; set; }

        [JsonProperty("cardType", NullValueHandling = NullValueHandling.Ignore)]
        public string CardType { get; set; }

        [JsonProperty("paymentProcessor")]
        public string PaymentProcessor { get; set; }

        [JsonProperty("expirationDate")]
        public string ExpirationDate { get; set; }

        [JsonProperty("transactionStatus")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionStatus TransactionStatus { get; set; }
    }
}
