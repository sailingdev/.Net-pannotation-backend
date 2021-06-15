using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class AddressResponseModel
    {
        [JsonProperty("country")]
        public string Country { get; set; }
        
        [JsonProperty("state")]
        public string State { get; set; }
        
        [JsonProperty("city")]
        public string City { get; set; }
        
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }
    }
}
