using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class StripeCreateSubscriptionRequestModel
    {
        [JsonProperty("planId")]
        [Required(ErrorMessage = "Is required")]
        public string PlanId { get; set; }
    }
}
