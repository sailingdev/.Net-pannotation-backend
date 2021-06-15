using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class RegisterUsingPhoneResponseModel
    {
        [JsonRequired]
        public string Phone { get; set; }

        [JsonRequired]
        public bool SMSSent { get; set; }
    }
}
