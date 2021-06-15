using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class AddCardRequestModel
    {
        [JsonProperty("userId")]
        [Required(ErrorMessage = "Is required")]
        public int UserId { get; set; }

        [JsonProperty("cardToken")]
        [Required(ErrorMessage = "Is required")]
        public string CardToken { get; set; }
    }
}
