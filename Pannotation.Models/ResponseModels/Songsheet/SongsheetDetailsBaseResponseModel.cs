using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels.Songsheet
{
    public class SongsheetDetailsBaseResponseModel : SongsheetBaseResponseModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("producer")]
        public string Producer { get; set; }

        [JsonProperty("arranger")]
        public string Arranger { get; set; }

        [JsonProperty("youTubeLink")]
        public string YouTubeLink { get; set; }

        [JsonProperty("preview")]
        public FileResponseModel Preview { get; set; }

        [JsonProperty("track")]
        public FileResponseModel Track { get; set; }
    }
}
