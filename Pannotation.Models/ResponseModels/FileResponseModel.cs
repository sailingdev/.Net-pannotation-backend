using Newtonsoft.Json;
using Pannotation.Models.Enums;

namespace Pannotation.Models.ResponseModels
{
    public class FileResponseModel: BaseFileResponseModel
    { 
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("type")]
        public FileType Type { get; set; }
    }
}
