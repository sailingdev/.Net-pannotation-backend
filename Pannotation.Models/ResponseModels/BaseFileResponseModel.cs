using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Pannotation.Models.ResponseModels
{
    public class BaseFileResponseModel
    {
        [Required]
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
