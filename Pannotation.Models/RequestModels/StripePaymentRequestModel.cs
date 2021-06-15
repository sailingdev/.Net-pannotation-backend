using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class StripePaymentRequestModel
    {
        [JsonProperty("cardToken")]
        [Required(ErrorMessage = "Is required")]
        public string CardToken { get; set; }

        [JsonProperty("amount")]
        [Required(ErrorMessage = "Is required")]
        [Range(0, long.MaxValue, ErrorMessage = "Is invalid")]
        public long Amount { get; set; }

        [JsonProperty("currency")]
        [Required(ErrorMessage = "Is required")]
        public string Currency { get; set; }
    }
}
