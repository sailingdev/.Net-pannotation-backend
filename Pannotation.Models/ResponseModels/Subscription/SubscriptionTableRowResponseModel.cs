using Newtonsoft.Json;
using System;

namespace Pannotation.Models.ResponseModels.Subscription
{
    public class SubscriptionTableRowResponseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("purchaseDate")]
        public DateTime? PurchaseDate { get; set; }

        [JsonProperty("nextPaymentDate")]
        public DateTime? NextPaymentDate { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }
}
