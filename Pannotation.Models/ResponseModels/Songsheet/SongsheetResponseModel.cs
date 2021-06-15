using Newtonsoft.Json;
using System.Collections.Generic;

namespace Pannotation.Models.ResponseModels.Songsheet
{
    public class SongsheetResponseModel : SongsheetDetailsBaseResponseModel
    {
        [JsonProperty("file")]
        public FileResponseModel File { get; set; }

        [JsonProperty("instruments")]
        public Dictionary<int, string> Instruments { get; set; }

        [JsonProperty("genres")]
        public Dictionary<int, string> Genres { get; set; }
    }
}
