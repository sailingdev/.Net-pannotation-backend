using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels
{
    public class CheckResetPasswordTokenResponseModel
    {
        [JsonRequired]
        public bool IsValid { get; set; }
    }
}