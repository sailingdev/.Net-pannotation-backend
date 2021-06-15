using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.RequestModels
{
    public class UserIdRequestModel
    {
        [JsonProperty("userId")]
        [Required(ErrorMessage = "UserId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId is invalid")]
        public int? UserId { get; set; }
    }
}
