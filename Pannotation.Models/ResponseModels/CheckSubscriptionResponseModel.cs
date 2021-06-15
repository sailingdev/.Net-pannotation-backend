using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class CheckSubscriptionResponseModel
    {
        [JsonProperty("isSubscribed")]
        public bool IsSubscribed { get; set; }

        [JsonProperty("startDate")]
        public string StartDate { get; set; }

        [JsonProperty("endDate")]
        public string EndDate { get; set; }
    }
}
