using Newtonsoft.Json;
using Pannotation.Models.ResponseModels.Payment;

namespace Pannotation.Models.ResponseModels.Subscription
{
    public class SubscriptionDetailsResponseModel : SubscriptionTableRowResponseModel
    {
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("idCode")]
        public string IdCode { get; set; }

        public TransactionInfoResponseModel TransactionInfo { get; set; }
    }
}
