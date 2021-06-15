using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class BraintreeSubscriptionRequestModel
    {
        [JsonProperty("paymentMethodToken")]
        [Required(ErrorMessage = "Is required")]
        public string PaymentMethodToken { get; set; }

        [JsonProperty("planId")]
        [Required(ErrorMessage = "Is required")]
        public string PlanId { get; set; }
    }
}