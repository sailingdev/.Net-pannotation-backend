using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels.OtherFiles
{
    public class OtherFileResponseModel : OtherFileBaseResponseModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("file")]
        public FileResponseModel File { get; set; }
    }
}
