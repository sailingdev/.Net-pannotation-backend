using Newtonsoft.Json;
using Pannotation.Models.ResponseModels.Base;

namespace Pannotation.Models.ResponseModels
{
    public class UserProfileResponseModel : UserProfileBaseResponseModel
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("mobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("isComposer")]
        public bool IsComposer { get; set; }

        [JsonProperty("idCode")]
        public string IdCode { get; set; }

        [JsonProperty("isSubscribed")]
        public bool IsSubscribed { get; set; }

        [JsonProperty("shouldCancelSubscription")]
        public bool ShouldCancelSubscription { get; set; }

        [JsonProperty("isBlocked")]
        public bool IsBlocked { get; set; }
    }
}
