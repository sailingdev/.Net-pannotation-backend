using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class TokenResponseModel
    {
        [JsonRequired]
        public string AccessToken { get; set; }

        [JsonRequired]
        public string RefreshToken { get; set; }

        [JsonRequired]
        public string ExpireDate { get; set; }

        [JsonRequired]
        public string Type { get; set; }
    }
}
