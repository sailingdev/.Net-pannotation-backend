using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels.Payment
{
    public class SignatureResponseModel
    {
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}
