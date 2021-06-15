using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class BraintreePaymentRequestModel
    {
        [JsonProperty("amount")]
        [Required(ErrorMessage = "Is required")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "Is invalid")]
        public decimal Amount { get; set; }

        [JsonProperty("nonce")]
        [Required(ErrorMessage = "Is required")]
        public string Nonce { get; set; }
    }
}
