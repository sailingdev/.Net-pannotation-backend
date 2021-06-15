using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class RegisterResponseModel
    {
        [JsonRequired]
        public string Email { get; set; }
    }
}