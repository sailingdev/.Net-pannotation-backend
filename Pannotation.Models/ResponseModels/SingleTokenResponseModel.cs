using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class SingleTokenResponseModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
