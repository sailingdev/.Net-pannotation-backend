using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class LoginResponseModel
    {
        [JsonRequired]
        public int Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        [JsonRequired]
        public string Role { get; set; }

        [JsonRequired]
        public TokenResponseModel Token { get; set; }
    }
}