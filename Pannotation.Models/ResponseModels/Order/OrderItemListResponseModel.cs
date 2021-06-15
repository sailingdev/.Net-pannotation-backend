using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pannotation.Models.ResponseModels.Order
{
    public class OrderItemListResponseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("buyerId")]
        public int BuyerId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("songsheets")]
        public List<string> Songsheets { get; set; }
    }
}
