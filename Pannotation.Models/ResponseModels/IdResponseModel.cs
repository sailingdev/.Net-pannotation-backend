using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class IdResponseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
