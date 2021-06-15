using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels.Base
{
    public class UserProfileBaseResponseModel : IdResponseModel
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("avatar")]
        public ImageResponseModel Avatar { get; set; }
    }
}
