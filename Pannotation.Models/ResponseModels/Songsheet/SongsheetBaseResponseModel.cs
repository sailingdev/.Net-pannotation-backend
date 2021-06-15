using Newtonsoft.Json;

namespace Pannotation.Models.ResponseModels.Songsheet
{
    public class SongsheetBaseResponseModel : IdResponseModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("artistName")]
        public string ArtistName { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("isTop")]
        public bool IsTop { get; set; }

        [JsonProperty("image")]
        public ImageResponseModel Image { get; set; }
    }
}
